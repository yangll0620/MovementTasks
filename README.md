# MovementTasks
Intergrate GoNogo, COT and Selfpace Tasks into one Program



## NMRC_GoNogoTask
Presentation in the touch screen of Go noGO task in NMRC
This is a C# Wpf version of the presentation program in the touch screen of the GonoGO task of NMRC. Only work in Windows as used C# WPF.

1. Multiple touch
2. Touch to start a new trial
3. Show go and nogo trials based on the preset ratio. 
	Total Trial Num/Position/Session: only the showed go or nogo trials counted 
		(i.e. if abandon during ready or cue phase, trialnum stays the same)



### Issues: (from Ziling)
1. Stop Bug.
2. com can't find when opened. Should be unplug and plug in.
3. Still has touched into gomiss cases
4. When input name, s can't be input as used for start hotkey.
5. Target 2 has no records.


### ToDo List
1. Change trial end defination into late(visual feedback ends, all touch point left)

7. Add Block Inf in saved file name

10. Resources debug/release used Problem.



### Advanced Functions:
1. Dynamic draw the target feedback grid accroding to the target num.
2. Automatically get the property name
3. Use table for each target realtime info feedback

### Easy way to do
1. Add the gonogotask_wpf .exe as for COTtask

