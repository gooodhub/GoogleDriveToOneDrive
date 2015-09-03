# GoogleDriveToOneDrive
Projet permettant d'exporter une arborescence à partir d'un identifiant de dossier depuis Google Drive vers une bibliothèque OneDrive.

Afin de lancer la solution :

1. Créer un fichier settings.config dans le dossier Exploitation (un fichier settings.config.sample est présentt dans l'aborescence, vous pouvez le copier en changeant son nom)
2. Celui-ci comporte un ensemble de noeuds, afin de faire fonctionner l'application avec votre environnement, il faudra le remplacer par vos propres données 

En cas de questions sur la récupération des informations de Google, vous pouvez consulter un tutoriel présent à cette adresse :  http://blog.softit.fr/post/google-drive-telecharger-et-convertir-ses-fichiers-par-la-programmation


a. accountServiceP12     ==> Correspond à la clé P12 vous permettant d'identifier votre compte de service google
b. accountServiceName ==>  nom du compte de service
c. GoogleLocalFolderStore  => Endroit où seront stockés localement vos documents google drive
d. ownerToFilter  => Propriétaire google sur lequel vous désirez filtrer
e. folderParentToRetrieve  => Identifiant dossier parent à récupérer (il est affiché dans l'url)
f. applicationName => Nom de votre application google
g. onedrive:destinationFolder => Nom du dossier de destination côté OneDrive
h. onedrive:urlSite  ==> Url du site OneDrive de destination
i. onedrive:login   ==> Compte de connexion à votre bibliothèque
onedrive:password ==> Mot de passe utiisateur ayant accès au site oneDrive de destination

