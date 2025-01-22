## Sensor experiments

Run the following command in your terminal: 

~~~
conda create -n mlagents python=3.10.12 -y
~~~

Changing sensors for scene: 
- Select soccerTwos scene in unity
- In the top left, click on the dropdown menu saying "SoccerTwos.unity"
- For each agent in SoccerTwos.unity, e.g. BlueStriker, you can click on their name and their associated scripts will pop up on the right. 
- Click the boxes next to each script to activate or deactivate them
- Save your preferred sensor settings

Run the following commands to start training your agents:

~~~
conda activate mlagents

pip install --upgrade pip

pip install mlagents

mlagents-learn (YOUR CONFIGURATIONS HERE)
~~~