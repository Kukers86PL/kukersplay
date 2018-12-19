#include <ImageSearch.au3>

global $y = 0, $x = 0
global $nick = FileReadLine("./login.txt")
global $host = FileReadLine("./hostip.txt")

while 1
	Local $search = _ImageSearch('h3newgamebutton.png', 1, $x, $y, 15)
	If $search = 1 Then
		MouseClick("left", $x, $y)
	EndIf
	$search = _ImageSearch('h3multiplayerbutton.png', 1, $x, $y, 15)
	If $search = 1 Then
		MouseClick("left", $x, $y)
	EndIf
	$search = _ImageSearch('h3nickedit.png', 1, $x, $y, 15)
	If $search = 1 Then
		MouseClick("left", $x, $y)
		For $i = 0 To 50 Step 1
			Send("{BS}");
		Next
		Send($nick);
	EndIf
	$search = _ImageSearch('h3tcpipbutton.png', 1, $x, $y, 15)
	If $search = 1 Then
		MouseClick("left", $x, $y)
	EndIf
	$search = _ImageSearch('h3joinbutton.png', 1, $x, $y, 15)
	If $search = 1 Then
		MouseClick("left", $x, $y)
	EndIf
	$search = _ImageSearch('h3hostipedit.png', 1, $x, $y, 15)
	If $search = 1 Then
		MouseClick("left", $x, $y)
		For $i = 0 To 50 Step 1
			Send("{BS}");
		Next
		Send($host);
	EndIf
	$search = _ImageSearch('h3okbutton.png', 1, $x, $y, 15)
	If $search = 1 Then
		MouseClick("left", $x, $y)
		ExitLoop
	EndIf
	sleep(1000)
WEnd