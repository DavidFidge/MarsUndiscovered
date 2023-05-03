param(
    [Parameter(Position=0,mandatory=$true)]
    [String]
    $FolderPath
)

Get-ChildItem "$FolderPath\*.png" -Recurse | ForEach-Object {
    $fileName = $_.FullName
    $split = ($fileName -split "\\")
    $outputPath = ""
    $found = $false;
    
    for ($i = 0; $i -lt $split.Length; $i++)
    {
        $str = ([string]$split[$i]).ToLower();
        if ($str -eq "content")
        {
            $found = $true
        }
        else {
            if ($found)
            {
                $outputPath += ([string]$split[$i])
                if ($i -lt $split.Length - 1)
                {
                    $outputPath += "/"
                }
            }
        }        
    }

    write-output "/importer:TextureImporter"
    write-output "/processor:TextureProcessor"
    write-output "/build:$outputPath"
    write-output ""
}
