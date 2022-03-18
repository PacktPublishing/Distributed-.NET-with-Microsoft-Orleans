$resourceGroup = "distelrg"
$location = "westus"
$storageAccount = "distelstorage"
$clusterName = "distelwebhost"
$containerRegistry = "distelwebhostacr2"

az login

# Create a resource group
az group create --name $resourceGroup --location $location

# Create an Azure storage account
az storage account create --location $location --name $storageAccount --resource-group $resourceGroup --kind "StorageV2" --sku "Standard_LRS"

# Create an AKS cluster. This can take a few minutes
az aks create --resource-group $resourceGroup --name $clusterName --node-count 3

# If you haven't already, install the Kubernetes CLI
az aks install-cli

# Authenticate the Kubernetes CLI
az aks get-credentials --resource-group $resourceGroup --name $clusterName

# Create an Azure Container Registry account and login to it
az acr create --name $containerRegistry --resource-group $resourceGroup --sku Standard

# Create a service principal for the container registry and register it with Kubernetes as an image pulling secret
$acrId = $(az acr show --name $containerRegistry --query id --output tsv)
$acrServicePrincipalName = "$($containerRegistry)-aks-service-principal"
$acrSpPw = $(az ad sp create-for-rbac --name http://$acrServicePrincipalName --scopes $acrId --role acrpull --query password --output tsv)

# 6fa017c7-c249-442d-9939-0bf1a1d3051d is the app id of http://distelwebhostacr2-aks-service-principal
$acrSpAppId = $(az ad sp show --id 6fa017c7-c249-442d-9939-0bf1a1d3051d --query appId --output tsv)
$acrLoginServer = $(az acr show --name $containerRegistry --resource-group $resourceGroup --query loginServer).Trim('"')
kubectl create secret docker-registry $containerRegistry --namespace default --docker-server=$acrLoginServer --docker-username=$acrSpAppId --docker-password=$acrSpPw

# Configure the storage account that the application is going to use by adding a new secret to Kubernetes
kubectl create secret generic az-storage-acct --from-literal=key=$(az storage account show-connection-string --name $storageAccount --resource-group $resourceGroup --output tsv)


$acrLoginServer = $(az acr show --name $containerRegistry --resource-group $resourceGroup --query loginServer).Trim('"')
az acr login --name $containerRegistry

docker tag distelwebhost:akshostdev distelwebhostacr2.azurecr.io/distelwebhost:v1

docker push distelwebhostacr2.azurecr.io/distelwebhost:v1

kubectl config get-contexts

kubectl config use-context distelwebhost

kubectl apply -f "DirectoryFolderPath\Distel.WebHost\deployment.yaml"

