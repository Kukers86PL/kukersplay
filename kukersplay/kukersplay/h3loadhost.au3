#include <ImageSearch.au3>
#include <GDIPlus.au3>
#include <File.au3>

Global $y = 0, $x = 0
Global $nick = FileReadLine("./login.txt")
Global $path = 'C:\Fraps\Screenshots'

Func _FindLatest($dir)
    Local $latest[2]
    Local $files = _FileListToArray($dir, "*", 1)
    For $i = 1 To UBound($files) - 1
        If FileGetTime($files[$i], 1, 1) > $latest[0] Or $i = 1 Then
            $latest[0] = FileGetTime($files[$i], 1, 1)
            $latest[1] = $i
        EndIf
    Next
	if $files then
		Return $files[$latest[1]]
	else
		Return ""
	endif
EndFunc

while 1
	WinActivate("Heroes of Might and Magic")
	MouseClick("left", 0, 0)
	Send("F10")
	Local $file = _FindLatest($path);
	Local $hbmp = _GDIPlus_BitmapCreateFromFile($file);
	Local $search = _ImageSearch('h3loadgamebutton.png', 1, $x, $y, 15, $hbmp)
	If $search = 1 Then
		MouseClick("left", $x, $y)
	EndIf
	$search = _ImageSearch('h3multiplayerbutton.png', 1, $x, $y, 15, $hbmp)
	If $search = 1 Then
		MouseClick("left", $x, $y)
	EndIf
	$search = _ImageSearch('h3nickedit.png', 1, $x, $y, 15, $hbmp)
	If $search = 1 Then
		MouseClick("left", $x, $y)
		For $i = 0 To 50 Step 1
			Send("{BS}");
		Next
		Send($nick);
	EndIf
	$search = _ImageSearch('h3tcpipbutton.png', 1, $x, $y, 15, $hbmp)
	If $search = 1 Then
		MouseClick("left", $x, $y)
	EndIf
	$search = _ImageSearch('h3hostbutton.png', 1, $x, $y, 15, $hbmp)
	If $search = 1 Then
		MouseClick("left", $x, $y)
	EndIf
	$search = _ImageSearch('h3okbutton.png', 1, $x, $y, 15, $hbmp)
	If $search = 1 Then
		MouseClick("left", $x, $y)
		ExitLoop
	EndIf
	sleep(2000)
WEnd