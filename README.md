mirror-interactions
=============

The interactions application for the Delta mirror project.



Build
---

__New cloned repository__

1. Go to -> build -> configuration manager.
Set mirror voice to -> anycpu and sacknet to x64

2. Build the project.
3. Go to -> references in the project, remove the sacknet reference.
4. Go to -> reference -> add reference, browse to the sacknet folder -> bin -> x64 -> debug and add "Sacknet.KinectFacialRecognition.dll".
5. Build the project.
6. Copy the vgbtechs folder with all contents to the Debug folder of the solution. 
You can find the vgbtechs folder here: C:\Program Files\Microsoft SDKs\Kinect\v2.0_1409\Tools\KinectStudio 




