# Autonomous Exploration Agent with Obstacle Avoidance 

## Brief

The project implements an autonomous agent searching for and moving towards a target, using reinforcement learning. The agent moves in a cluttered randomly generated space and is incentivated to avoid obstacles in its path or exploration towards the target.

## Structure

The repository is structured as follows:

- [`src`](src) contains:
    - [`config`](src/config) which holds the hyperparameter configurations for PPO and the curricula for Curriculum Learning
    - [`notebooks`](src/notebooks) which holds notebooks for Jupyter for testing and extracting data from the `mlagents` Python package (which contains the actual RL implementations)
    - [`UnitySDK`](src/UnitySDK) holds Unity's source code for the environment, consisting of scenes, agent and other classes, etc.
- [`docs`](docs) contains a formalized report of the project and a presentation.

## Prerequisites

* Unity v2019.1 or higher
* [ML-Agents v0.11](https://github.com/Unity-Technologies/ml-agents)
* Python 3.6

## Installation
```sh
$ git clone https://github.com/dodicin/autonomous-exploration-agent
$ cd autonomous-exploration-agent
$ virtualenv .venv -p C:/Python/Python36/python.exe 
$ .venv/Script/activate.bat
$ pip install mlagents
```

If pip gives some errors for the bleach and pillow packages:
```sh
$ pip uninstall pillow==6.1.0 // Uninstall the uncompatible version
$ pip install pillow==5.4.1

$ pip uninstall bleach==3.1.0
$ pip install bleach==1.5.0
```

## Training
* Open the project
* Tick the Control checkboxes in the Broadcast Hub for the Brains that are going to be trained
* Remove the model from the Brain objects under `./Assets/Brains`
* Launch a command prompt:
```sh
$ tensorboard --logdir=summaries
$ mlagents-learn src\config\trainer_config.yaml --curriculum=src\config\curricula\autonomous-exploration-agent\ --run-id=<RunID> --train
```
* Stop the training when the cumulative reward reaches a satisfying level
* Models are saved under `./models`

# Testing
* Open the project
* Unick the Control checkboxes in the Broadcast
* Add the models to the Brain objects under `./Assets/Brains`. The models are found under `./models`
* Press the play button to see the trained model in action

## Authors

* **Federico Bottoni** (806944) - [FedericoBottoni](https://github.com/FedericoBottoni)
* **Nassim Habbash** (808292) - [nhabbash](https://github.com/dodicin)
