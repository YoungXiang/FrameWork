@echo off

set ExcelRoot=%~dp0\Client\
set JlsRoot=%ExcelRoot%..\..\..\Assets\StreamingAssets\ConfigDatas\
set CsRoot=%ExcelRoot%..\..\..\Assets\Scripts\ConfigDatas\

echo ��ʼ����

echo ExcelRoot = %ExcelRoot%
echo CsRoot = %CsRoot%
echo JlsRoot = %JlsRoot%

pushd %~dp0

BinaryDataTool\JsonSharp.exe %ExcelRoot% %CsRoot% %JlsRoot%

echo �������

pause