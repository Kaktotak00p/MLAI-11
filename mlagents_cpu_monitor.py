import subprocess
import argparse
import sys
from pathlib import Path

def run_mlagents(in_editor_mode, env_path):
    """
    Starts mlagents-learn with the appropriate options and returns the subprocess handle.
    """
    run_id_suffix = "in_editor" if in_editor_mode else "precompiled"
    command = [
        "mlagents-learn",
        "config/poca/SoccerTwos.yaml",
        f"--run-id=finalRun{run_id_suffix}",
    ]

    if not in_editor_mode:
        if not Path(env_path).is_file():
            print(f"Error: The provided environment path '{env_path}' does not exist or is not a file.")
            return None
        command.append("--no-graphics")
        command.append(f"--env={env_path}")

    print(f"Running ML-Agents with command:\n  {' '.join(command)}\n")
    try:
        process = subprocess.Popen(command, stdout=sys.stdout, stderr=sys.stderr)
    except FileNotFoundError:
        print("Error: mlagents-learn not found. "
              "Please ensure it's in your PATH or installed in your current environment.")
        return None

    return process

def main():
    parser = argparse.ArgumentParser(description="Run ML-Agents training.")
    parser.add_argument("-e", "--editor", action="store_true",
                        help="Run training in the Unity Editor mode (no --env).")
    parser.add_argument("--env", type=str,
                        default="./Project/build/testBuild.x86_64",
                        help="Path to the compiled Unity environment (if not using Editor).")
    args = parser.parse_args()

    in_editor_mode = args.editor
    env_path = args.env

    # Launch ML-Agents
    mlagents_process = run_mlagents(in_editor_mode, env_path)
    if mlagents_process is None:
        return

    # Ensure ML-Agents has fully exited
    if mlagents_process.poll() is None:
        mlagents_process.wait()

    print("\nTraining finished successfully.\n")

if __name__ == "__main__":
    main()
