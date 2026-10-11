# Original calendar artwork for this project, released under the repository MIT license.
# Draw vector shapes directly; no external artwork, fonts or icon libraries are used.
param([switch]$Preview)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$repository = Split-Path $PSScriptRoot -Parent
$output = Join-Path $repository 'WeekTrayInfo\Resources\CalendarIcon.ico'

function New-RoundedRectangle([float]$x, [float]$y, [float]$width, [float]$height, [float]$radius) {
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $diameter = 2 * $radius
    $path.AddArc($x, $y, $diameter, $diameter, 180, 90)
    $path.AddArc($x + $width - $diameter, $y, $diameter, $diameter, 270, 90)
    $path.AddArc($x + $width - $diameter, $y + $height - $diameter, $diameter, $diameter, 0, 90)
    $path.AddArc($x, $y + $height - $diameter, $diameter, $diameter, 90, 90)
    $path.CloseFigure()
    return $path
}

$images = New-Object 'System.Collections.Generic.List[byte[]]'
$sizes = @(16, 20, 24, 32, 48, 64, 128, 256)
foreach ($size in $sizes) {
    $bitmap = New-Object System.Drawing.Bitmap($size, $size)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $stream = New-Object System.IO.MemoryStream
    $navy = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml('#18344D'))
    $paper = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml('#F5F8FC'))
    $teal = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml('#087E8B'))
    $muted = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml('#B7C8D6'))
    try {
        $graphics.Clear([System.Drawing.Color]::Transparent)
        $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
        $graphics.ScaleTransform($size / 64.0, $size / 64.0)
        $outline = New-RoundedRectangle 5 9 54 51 7
        $inside = New-RoundedRectangle 9 13 46 43 4
        $header = New-RoundedRectangle 9 13 46 16 4
        try {
            $graphics.FillPath($navy, $outline)
            $graphics.FillPath($paper, $inside)
            $graphics.FillPath($teal, $header)
            $graphics.FillRectangle($teal, 9, 23, 46, 6)
        } finally { $outline.Dispose(); $inside.Dispose(); $header.Dispose() }
        foreach ($x in @(17, 42)) {
            $binding = New-RoundedRectangle $x 4 5 15 2
            try { $graphics.FillPath($navy, $binding) } finally { $binding.Dispose() }
        }
        # One highlighted week is the central visual motif.
        $week = New-RoundedRectangle 13 33 38 8 2
        try { $graphics.FillPath($teal, $week) } finally { $week.Dispose() }
        for ($column = 0; $column -lt 7; $column++) {
            $x = 15 + 5 * $column
            $graphics.FillRectangle($paper, $x, 35, 2, 4)
            $graphics.FillRectangle($muted, $x, 46, 2, 4)
        }
        $bitmap.Save($stream, [System.Drawing.Imaging.ImageFormat]::Png)
        $images.Add($stream.ToArray())
        if ($Preview -and $size -eq 256) {
            $previewDirectory = Join-Path $repository 'artifacts'
            New-Item -ItemType Directory -Path $previewDirectory -Force | Out-Null
            $bitmap.Save((Join-Path $previewDirectory 'calendar-icon-preview.png'), [System.Drawing.Imaging.ImageFormat]::Png)
        }
    } finally {
        $graphics.Dispose(); $bitmap.Dispose(); $stream.Dispose()
        $navy.Dispose(); $paper.Dispose(); $teal.Dispose(); $muted.Dispose()
    }
}

# ICO directory followed by PNG frames, supported by Windows Vista and later.
$file = [System.IO.File]::Create($output)
$writer = New-Object System.IO.BinaryWriter($file)
try {
    $writer.Write([uint16]0); $writer.Write([uint16]1); $writer.Write([uint16]$sizes.Count)
    $offset = 6 + 16 * $sizes.Count
    for ($index = 0; $index -lt $sizes.Count; $index++) {
        $dimension = if ($sizes[$index] -eq 256) { 0 } else { $sizes[$index] }
        $writer.Write([byte]$dimension); $writer.Write([byte]$dimension)
        $writer.Write([byte]0); $writer.Write([byte]0)
        $writer.Write([uint16]1); $writer.Write([uint16]32)
        $writer.Write([uint32]$images[$index].Length); $writer.Write([uint32]$offset)
        $offset += $images[$index].Length
    }
    foreach ($bytes in $images) { $writer.Write([byte[]]$bytes) }
} finally { $writer.Dispose(); $file.Dispose() }
Write-Output "Icon: $output"
