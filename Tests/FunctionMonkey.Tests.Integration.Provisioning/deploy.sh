# az group create -l westeurope -n functionMonkeyIntegration
# az group deployment create -g functionMonkeyIntegration --template-file azuredeploy.json

echo "** Creating Storage Containers"
STORAGEACCOUNTNAME=`az storage account list --resource-group functionMonkeyIntegration --query '[0].name'`
STORAGEACCOUNTNAME="${STORAGEACCOUNTNAME%\"}"
STORAGEACCOUNTNAME="${STORAGEACCOUNTNAME#\"}"
CONNECTIONSTRING=`az storage account show-connection-string -g functionMonkeyIntegration -n "$STORAGEACCOUNTNAME" --query connectionString`
CONNECTIONSTRING="${CONNECTIONSTRING%\"}"
CONNECTIONSTRING="${CONNECTIONSTRING#\"}"
az storage queue create --name myqueue --connection-string $CONNECTIONSTRING
az storage container create --name blobcommandcontainer --connection-string $CONNECTIONSTRING
az storage container create --name streamblobcommandcontainer --connection-string $CONNECTIONSTRING

COSMOSDBNAME=`az cosmosdb list --resource-group functionMonkeyIntegration --query "[0].name"`
COSMOSDBNAME="${COSMOSDBNAME%\"}"
COSMOSDBNAME="${COSMOSDBNAME#\"}"
COSMOSDBKEY=`az cosmosdb list-keys --name $COSMOSDBNAME --resource-group functionMonkeyIntegration --query primaryMasterKey`
COSMOSDBKEY="${COSMOSDBKEY%\"}"
COSMOSDBKEY="${COSMOSDBKEY#\"}"

COSMOSDATABASEEXISTS=`az cosmosdb database exists --db-name cosmosDatabase --key "$COSMOSDBKEY" --name "$COSMOSDBNAME" --resource-group-name functionMonkeyIntegration`
if [[ "$COSMOSDATABASEEXISTS" == "false" ]]; then
    echo "** CREATING COSMOS DATABASE"
    az cosmosdb database create --db-name cosmosDatabase --key "$COSMOSDBKEY" --name "$COSMOSDBNAME" --resource-group-name functionMonkeyIntegration
fi
COSMOSCOLLECTIONEXISTS=`az cosmosdb collection exists --db-name cosmosDatabase --collection-name cosmosCollection --key "$COSMOSDBKEY" --name "$COSMOSDBNAME" --resource-group-name functionMonkeyIntegration`
if [[ "$COSMOSCOLLECTIONEXISTS" == "false" ]]; then
    echo "** CREATING COSMOS COLLECTION"
    az cosmosdb collection create --db-name cosmosDatabase --collection-name cosmosCollection --key "$COSMOSDBKEY" --name "$COSMOSDBNAME" --resource-group-name functionMonkeyIntegration --throughput 400 --partition-key-path "/id"
fi