Fuzzy Logic Unity Editor

For this project I created a Unity utility that allows you to edit fuzzy numbers easily in the editor and attach them to other scripts. Included are screenshots of the editor, as well as the source and a sample level demo demonstrating the tools in action.

The included demo has a simple player character - a blue person, and "Enemies", red people who just stand there. The fuzzy was used to give the camera a useful set of transitions between "cinematic" and "action" modes when the player gets close to an enemy.

The linguistic variable definitions for the camera are as follows:

Ideal Distance:
	Close up	(0, 3, 6)
	Mid Range 	(5, 15, 25)
	Far Out 	(20, 30, 40)

Damping Time:
	Quick 		(-1, 0, 1)
	Slow		(1, 2, 2.1)

Look Speed:
	Action		(15, 30, 40)
	Smooth		(0, 10, 35)

Move Speed:
	Fast		(10, 30, 40)
	Cinematic 	(0, 5, 30)

Additionally, the character has a linguistic variable for the closest enemy.

Closest:
	Near		(-0.5, 2, 10)
	Medium		(0, 20, 30)
	Far			(15, 30,50)

The rule base component on the camera take these linguistic variables and a set of rules, and reads and sets the corresponding variables accordingly each frame.

Rules
	When Closest is Near		IdealDistance is Close Up
	When Closest is Medium		IdealDistance is  Midrange
	When Closest is Far			IdealDistance is Far Out

	When Closest is Near		Damping Time is Quick
	When Closest is Far 		Damping Time is Slow

	When Closest is Far 		Look Speed is Smooth
	When Closest is Near		Look Speed is Action
	
	When Closest is Far 		Move Speed is Cinematic
	When Closest is Near		Move Speed is Fast

