@echo off
cd solr-1.4.0
md logs
start C:\Windows\System32\java -jar start.jar
cd ..
cscript /nologo pingsolr.js
