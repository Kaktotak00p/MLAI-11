import os
import subprocess
import time
import yaml
import optuna
import json
import psutil
from pathlib import Path
from tbparse import SummaryReader

def create_temp_trainer_config(trial, base_config_path="./config/poca/SoccerTwos.yaml"):
    """
    Reads a base trainer config, updates it with hyperparams from `trial`,
    and writes out a temporary config file.
    """
    # --- Suggest hyperparameters from Optuna ---
    learning_rate = trial.suggest_float("learning_rate", 1e-5, 1e-3, log=True)
    batch_size    = trial.suggest_categorical("batch_size", [512, 1024, 2048])
    buffer_size   = trial.suggest_categorical("buffer_size", [2048, 4096, 8192])
    beta          = trial.suggest_float("beta", 0.0001, 0.01)
    num_layers    = trial.suggest_int("num_layers", 1, 3)
    hidden_units  = trial.suggest_categorical("hidden_units", [128, 256, 512])

    # --- Load the base config ---
    with open(base_config_path, "r") as f:
        config = yaml.safe_load(f)

    # Access soccer config at: behaviors -> SoccerTwos
    soccer_config = config["behaviors"]["SoccerTwos"]

    # POCA "hyperparameters" block for aspects like batch_size, buffer_size, etc.
    soccer_config["hyperparameters"]["learning_rate"] = learning_rate
    soccer_config["hyperparameters"]["batch_size"]    = batch_size
    soccer_config["hyperparameters"]["buffer_size"]   = buffer_size
    soccer_config["hyperparameters"]["beta"]          = beta

    # POCA "network_settings" block for hidden_units, num_layers, etc.
    soccer_config["network_settings"]["hidden_units"] = hidden_units
    soccer_config["network_settings"]["num_layers"]   = num_layers
    soccer_config["max_steps"] = 500_000  # e.g., 500k steps

    # --- Create a temporary trainer config file ---
    temp_config_path = f"temp_trainer_config_trial_{trial.number}.yaml"
    with open(temp_config_path, "w") as f:
        yaml.dump(config, f)

    return temp_config_path


def run_training_and_get_score(temp_config_path, trial, env_path="./Project/build/testBuild.x86_64"):
    """
    Runs mlagents-learn with the temporary config file and returns a performance metric.
    Also captures CPU/memory usage of the 'mlagents-learn' process via psutil
    and saves console output to a log file for parsing ELO (or other metrics).
    """
    run_id = f"soccer_twos_trial_{trial.number}"
    results_dir = Path("results") / run_id
    results_dir.mkdir(parents=True, exist_ok=True)

    # We'll capture console output to results_dir/training_console.log
    console_log_path = results_dir / "training_console.log"

    # Construct the command
    cmd = [
        "mlagents-learn", temp_config_path,
        f"--env={env_path}",
        f"--run-id={run_id}",
        "--no-graphics",
        "--force",
    ]

    print(f"\n=== Starting training for Trial #{trial.number} ===")
    print(f"Command: {' '.join(cmd)}\n")

    # Open a file handle for the console output
    with open(console_log_path, "w") as console_file:
        # Use Popen so we can poll resource usage via psutil
        process = subprocess.Popen(cmd, stdout=console_file, stderr=console_file)

        # psutil process handle (for resource usage)
        ps_handle = psutil.Process(process.pid)

        # Create or open a CSV-like log file for profiling
        profiler_log_path = results_dir / "profiler_usage.csv"
        with open(profiler_log_path, "w") as fprof:
            fprof.write("time_sec,cpu_percent,mem_mb\n")

            # Continuously poll until process finishes
            while True:
                # Check if the process is done
                ret_code = process.poll()
                if ret_code is not None:
                    # Training process ended
                    break

                # Gather CPU/memory usage for this specific process
                cpu_usage = ps_handle.cpu_percent(interval=0.0)  # immediate usage
                mem_info  = ps_handle.memory_info()             # returns RSS, VMS, etc.
                mem_mb    = mem_info.rss / (1024 * 1024)        # convert bytes to MB

                # Log to the file
                elapsed_sec = int(time.time())
                fprof.write(f"{elapsed_sec},{cpu_usage:.2f},{mem_mb:.2f}\n")
                fprof.flush()

                # Sleep for 1 second between polls
                time.sleep(1)

        # If the process returned a non-zero code, treat as a failed trial
        if process.returncode != 0:
            print(f"Training run failed for trial {trial.number} (return code: {process.returncode})")
            return -9999.0

    # Sleep a bit to ensure logs are written
    time.sleep(2)

    # -- Parse the final performance metric from the console log --
    final_metric = parse_from_console(console_log_path)
    return final_metric


def parse_from_console(log_path: Path) -> float:
    """
    Reads the final ELO (or other metric) from the ML-Agents console output file.
    For example, lines look like:
      [INFO] SoccerTwos. Step: 180000. ... ELO: 1216.314.
    We'll parse the last ELO we encounter in the file.
    """
    if not log_path.exists():
        print(f"Log file not found: {log_path}")
        return 0.0

    final_elo = 0.0
    with open(log_path, "r") as f:
        for line in f:
            # Example line:
            # [INFO] SoccerTwos. Step: 180000. Time Elapsed: 240.226 s. Mean Reward: 0.500. ...
            # Training. ELO: 1216.314.
            if "ELO:" in line:
                # We'll assume the ELO follows "ELO:"
                parts = line.split("ELO:")
                if len(parts) > 1:
                    # Try to parse the number after "ELO:"
                    try:
                        possible_elo = parts[1].strip().split()[0]  # e.g. "1216.314."
                        # Also remove any trailing punctuation like "." or ",":
                        possible_elo = possible_elo.rstrip(".,;")
                        final_elo = float(possible_elo)
                    except ValueError:
                        continue

    print(f"Parsed final ELO from console logs: {final_elo}")
    return final_elo


def objective(trial):
    # 4.1) Create the trainer config for this trial
    temp_config_path = create_temp_trainer_config(
        trial,
        base_config_path="./config/poca/SoccerTwos.yaml"
    )

    # 4.2) Run the training with profiling (and console capture)
    final_score = run_training_and_get_score(
        temp_config_path,
        trial,
        env_path="./Project/build/testBuild.x86_64"  # or another path
    )

    # 4.3) Cleanup the temp config file if desired
    if os.path.exists(temp_config_path):
        os.remove(temp_config_path)

    # Return the final ELO as our score
    return final_score

def main():
    study_name = "soccer_twos_bayesian_study"
    storage_db = "sqlite:///soccer_twos_study.db"  # local SQLite DB

    # Create or load an existing study
    study = optuna.create_study(
        study_name=study_name,
        storage=storage_db,
        direction="maximize",  # because we're maximizing ELO
        load_if_exists=True
    )

    # Start the Bayesian optimization
    study.optimize(objective, n_trials=20)

    # Print the best result
    print(f"Best value (ELO): {study.best_value}")
    print("Best hyperparameters:")
    for k, v in study.best_params.items():
        print(f"  {k}: {v}")


if __name__ == "__main__":
    main()

