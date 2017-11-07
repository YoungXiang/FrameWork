@echo off

set ExcelRoot=%~dp0\Client\
set JlsRoot=%ExcelRoot%..\..\..\Assets\StreamingAssets\ConfigDatas\
set CsRoot=%ExcelRoot%..\..\..\Assets\Scripts\ConfigDatas\

echo 开始导表

echo ExcelRoot = %ExcelRoot%
echo CsRoot = %CsRoot%
echo JlsRoot = %JlsRoot%

pushd %~dp0

BinaryDataTool\JsonSharp.exe %ExcelRoot% %CsRoot% %JlsRoot%

echo 导表完成

pause