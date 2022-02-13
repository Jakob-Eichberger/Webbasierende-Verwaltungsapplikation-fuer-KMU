# Webbasierende Verwaltungsapp f�r KMUs

## Einrichten des Repositories

Erstelle in der Kosole einen Ordner und synchronisiere erstmals das Repository:

```
Path>md KmuManager
Path>cd KmuManager
Path>git init
Path>git remote add origin https://github.com/IngMahalo/Webbasierende-Verwaltungsapplikation-fuer-KMU.git
Path>git fetch --all
Path>git reset --hard origin/master
```

## Arbeit mit dem Repository

Es sind 2 cmd Dateien enthalten:

- *resetGit.cmd*: L�scht alle lokalen �nderungen und setzt das lokale Repository auf dem Stand des
  Servers zur�ck (Vorsicht dabei!).
- *syncGit.cmd*: F�hrt ein automatisches Commit in den Master Branch durch.
