@echo off
cd solr-1.4.0
md logs
start C:\Windows\SysWOW64\java -jar start.jar
cd ..
cscript /nologo pingsolr.js
