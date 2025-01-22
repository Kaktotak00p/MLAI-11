## Sensor experiments

Run the following command in your terminal: 

~~~bash
conda create -n mlagents python=3.10.12 -y
~~~

Changing sensors for scene: 
- Select soccerTwos scene in unity
- In the top left, click on the dropdown menu saying "SoccerTwos.unity"
- For each agent in SoccerTwos.unity, e.g. BlueStriker, you can click on their name and their associated scripts will pop up on the right. 
- Click the boxes next to each script to activate or deactivate them
- Save your preferred sensor settings

Run the following commands to start training your agents:

~~~bash
conda activate mlagents

pip install --upgrade pip

pip install mlagents optuna psutil

mlagents-learn (YOUR CONFIGURATIONS HERE)
~~~

---
## Hyperparameter optimization

Run the following command to start the hyperparameter optimization:

~~~bash 
pyhton optimiser.py
~~~

The optimizer script will run for 20 iterations, you can stop it manually or wait for it to finish.
After the script is stopped you can inspect the result in the ./results/ folder, or continue running the optimizer from the point it was stopped.
The script uses SQLite to track different runs.

---

## Hyperparameter Experiments

To change the parameter values manually go to MLAI-11/config/poca/SoccerTwos.yaml. 
This has the default configuration for SoccersTwo.
Change any parameter you'd like.
After the change run the command:
~~~bash 
mlagents-learn ~/MLAI-11/config/poca/SoccerTwos.yaml
~~~

---
## In editor vs precompiled testing

Run the following command to start the training using the in editor environment. The script will track the cpu and memory usage of mlagents-learn.
~~~bash
python mlagents_cpu_monitor.py -e
~~~

To run the training using the precompiled env first build the Unity scene. In order to do that, open
 the Unity Editor, go to File-> Build Settings...-> Add
 open scenes and add the correct scene - SoccerTwos.
 Then click Build.  Once built, provide the path to the environment using the --env flag.

~~~bash
python mlagents_cpu_monitor.py --env path/to/your/build
~~~

If no flag is provided, the script will try to run the ./Project/build/testBuild.x86_64 build 

If you want to just run the training, you have to put a command in the terminal:

for in-editor:
~~~bash
 mlagents-learn (path to)SoccerTwost.yaml \--run-id=[our run id]
 ~~~

for pre-compiled:
~~~bash
 mlagents-learn (path to) \SoccerTwos.yaml \--env (path to)\UnityEnvironment.exe \--run-id=[our run id] \--no-graphics
 ~~~

"--no-graphics" is not nessesery, but If you add this, the graphics for the scene are not generated which saves some compute.
Specify the path to the yaml file responsible
 for the training (SoccerTwos.yaml) as well as the path to the compiled scene (UnityEnvironment.exe) and supply
 the run-id as the name of the trial. If you are doing in-editor training, after entering the command, play the scene in the Unity Editor.

