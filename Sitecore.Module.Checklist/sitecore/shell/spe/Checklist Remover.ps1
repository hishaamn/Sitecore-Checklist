$item = Get-Item .
	
$response = Show-Confirm -Title "Are you sure you want to remove the checklist $($item.Name)?"

if($response -ne "yes"){
	Exit
}

$itemName = $item.Name
$reportPath = "/sitecore/system/Modules/Checklist/Reports/$itemName"

Remove-Item $item.Paths.FullPath -Recurse
Remove-Item $reportPath -Recurse

Invoke-JavaScript -Script "window.parent.Sitecore.Speak.app.getItemList(); window.parent.Sitecore.Speak.app.SubAppRendererChecklistDetail.DialogWindow.hide();"