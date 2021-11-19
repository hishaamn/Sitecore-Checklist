$input = Show-Input "Enter checklist name"

if($input -eq $null){
	Exit
} else{
	$checklistItem = New-Item -Path "master:/sitecore/system/Modules/Checklist/Scripts" -Name $input -ItemType "{C6183877-163D-4FC6-8B97-A2358D1A2FD3}"
	
	$response = Show-FieldEditor -Item $checklistItem -Name "Rule" -Title "Checklist Editor for $($checklistItem.Name)"
	
	Invoke-JavaScript -Script "window.parent.Sitecore.Speak.app.getItemList();"	
}