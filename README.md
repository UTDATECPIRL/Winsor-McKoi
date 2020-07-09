# Winsor McKoi 
Winsor McKoi Interactive Fish Project is an experiment in delight and engagement. Using Leap Motion Controller and Ultrasonic Sensor as well as a Looking Glass holographic display, the window-front installation presents an animated origami fish, named Winsor McKoi. The virtual fish responds in real-time to physical presence and gestures (e.g., sprinkle, wave, and point) as well as tweets in order to cultivate a community of participants who actively engage in maintaining the “life” of the virtual fish over time. In doing so, Winsor McKoi Interactive Fish Project imagines that post-COVID-19 interfaces might be “touching” in ways that do not require fingers.

## Getting Started
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. 

### Prerequisites 
* Windows 10 or macOS(The COM port in the Presence GameObject would need to be modified accordingly)
* [Leap Motion Controller](https://www.ultraleap.com/product/leap-motion-controller/)
* Arduino Uno and HR-SC04 Ultrasonic Sensor

### Installation
Clone this repository or download the zip.
```
https://github.com/UTDATECPIRL/Winsor-McKoi.git
```
#### Unity Component
The UI for this installation is built in Unity. Alembic animations of Winsor react to input from the Leap software, as well as from the ML and Presence Detection components. Winsor is made more "alive" with systems to track happiness, fullness, and his tank's dirtiness. 
* Install [Unity version 2019.3.9f1](https://store.unity.com/#plans-individual)
* Install [Leap Motion for Unity](https://developer.leapmotion.com/unity#5436356) 
* (Compiler errors may occur with Alembic in the Unity Inspector. If this occurs, install [Alembic for Unity](https://docs.unity3d.com/Packages/com.unity.formats.alembic@1.0/manual/index.html))

#### Machine Learning Component
The ML model takes the finger coordinates from the leap motion controller as input. A DTW algorithm is used to perform time series analysis on the incoming temporal sequence. The similarity percentage thus obtained is used to decide which gesture is being performed by the user in real time.   
* Install [Wekinator](http://www.wekinator.org/downloads/)

#### Presence Detection Component:
We are using an Arduino Uno with an ultrasonic sensor to detect physical presence in front of the installation. A person's presence is used to trigger the fish to fold up into its idle state which is intended to encourage the user to interact further with installation. 
* Install the [Arduino IDE](https://www.arduino.cc/en/main/software)
* Assemble the circuit using the following diagram. Connect the trigger pin to D6 and the echo pin to D5 on the arduino uno.
![Arduino Connections](https://hackster.imgix.net/uploads/attachments/991561/uploads2ftmp2ff6c8de93-288c-4663-9a29-31c8e61172812fultrasonic5_WCDWvutJmv.png?auto=compress%2Cformat&w=1280&h=960&fit=max)
* Upload the code (Winsor-McKoi/Presence_Detection) to the arduino. 
* Open the Fish Scene in the Unity Project from the folder (Winsor-McKoi/Unity_Project) and change the COM port in the Presence GameObject to the port selected in the Arduino IDE. 

## Running the application
* Open the .wekproj file in the folder (Winsor-McKoi/ML_Component) from within the wekinator application and click run.
* Run the Unity application in the folder (Winsor-McKoi/Unity_Application). Follow the guide below to explore the fish's behaviours. 

### Demo Guide:
The project launches in the standing paper mode and then waits for presence to be detected by the ultrasonic sensor. Once presence is detected it allows the user to interact with the remaining gestures. If you are running the demo without the arduino set up, press the keyboard key "T" to get past the presence detection stage and move on to hand gesture recognition. 

**Input 1 (Keyboard Keys)** | **Input 2 (Gestures)** | **Action**
------------ | ------------- | ------------ 
 T | Presence detected | Fold up into idle state
 W | Wave | Reduce onscreen dirt
 S | Sprinkle Food | Go up to feed
 P | Point finger | Shy or curious behavior (Happiness dependant)

## Built With
* [Leap Motion](https://developer.leapmotion.com/unity) - Hand Recognition
* [Wekinator](http://www.wekinator.org/) - Machine Learning Component
* [Unity](https://unity.com/) - UI component
* [Arduino](https://www.arduino.cc/en/main/software) - Presence Detection Component
* [Autodesk Maya](https://www.autodesk.com/products/maya/overview?support=ADVANCED&plc=MAYA&term=1-YEAR&quantity=1) - Animations 

## Contributors and Authors

### Project Team
* **Heidi Rae Cooley**
* **Dale MacDonald** - [mrdale1958](https://github.com/mrdale1958?tab=repositories)
* **Danai Bavishi** - [danaibavishi](https://github.com/danaibavishi)
* **Mason McCully** - [Nosam55](https://github.com/Nosam55)
* **Taylor Lawson**
* **Sean Landers** - [seanlanders](https://github.com/seanlanders)

### Animations Team
* **Christine Veras**
* **Samuel Price**
* **Julio Soto** - [juliocsoto](https://juliocsoto.com/)


## License
This project is licensed under the CC License - see the [LICENSE.md](LICENSE.md) file for details
