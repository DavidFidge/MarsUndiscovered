Get-ChildItem *.cs -Recurse | ForEach-Object {
    $fileContents = (Get-Content $_.FullName)
    write-output $fileContents[0]
    if ($fileContents[0] -eq "")
    {
        write-output "replacing $($_.FullName)"
        (Get-Content $_.FullName) | Select-Object -Skip 1 | Set-Content $_.FullName
    }
}