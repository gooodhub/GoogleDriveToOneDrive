# GoogleDriveToOneDrive
Projet permettant d'exporter une arborescence à partir d'un identifiant de dossier depuis Google Drive vers une bibliothèque OneDrive.

Afin de lancer la solution :

1. Créer un fichier settings.config dans le dossier Exploitation (un fichier settings.config.sample est présentt dans l'aborescence, vous pouvez le copier en changeant son nom)
2. Celui-ci comporte un ensemble de noeuds, afin de faire fonctionner l'application avec votre environnement, il faudra le remplacer par vos propres données 

En cas de questions sur la récupération des informations de Google, vous pouvez consulter un tutoriel présent à cette adresse :  http://blog.softit.fr/post/google-drive-telecharger-et-convertir-ses-fichiers-par-la-programmation
<configuration>
  <add key="accountServiceP12" value="C:\GoogleDrive.p12"/>    ==> Correspond à la clé P12 vous permettant d'identifier votre compte de service google
  <add key="accountServiceName" value ="someGuid@developer.gserviceaccount.com" />  ==>  nom du compte de service
  <add key="GoogleLocalFolderStore" value="C:\GoogleDrive\"/> => Endroit où seront stockés localement vos documents google drive
  <add key="ownerToFilter" value="c.burceaux@clt-services.com"/> => Propriétaire google sur lequel vous désirez filtrer
  <add key="folderParentToRetrieve" value="id"/> => Identifiant dossier parent à récupérer (il est affiché dans l'url)
  <add key="applicationName" value="GoogleDriveToOneDrive"/> => Nom de votre application google
  <add key="onedrive:destinationFolder" value="ced"/>  => Nom du dossier de destination côté OneDrive
  <add key="onedrive:urlSite" value ="https://cltservices365-my.sharepoint.com/personal/c_burceaux_cltservices365_onmicrosoft_com"/>  ==> Url du site OneDrive de destination
  <add key="onedrive:login" value="c.burceaux@cltservices365.onmicrosoft.com"/>  ==> Compte de connexion à votre bibliothèque
  <add key="onedrive:password" value="pass"/> ==> Mot de passe utiisateur ayant accès au site oneDrive de destination
</configuration>
