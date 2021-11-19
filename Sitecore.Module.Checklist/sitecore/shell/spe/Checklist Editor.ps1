$item = Get-Item .

$response = Show-FieldEditor -Item $item -Name "Rule" -Title "Checklist Editor"

if($response -ne "Cancel"){
	Invoke-JavaScript -Script "window.parent.Sitecore.Speak.app.SubAppRendererChecklistDetail.loadChecklist()"
}