#include <ImageSearch.au3>

global $y = 0, $x = 0

Func checkForImage()
Local $search = _ImageSearch('checkImage.png', 15, $x, $y, 1)
If $search = 1 Then
MouseMove($x, $y, 10)
EndIf
EndFunc

checkForImage()
