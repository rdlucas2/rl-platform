install git
clone repo

install dapr

on windows (in admin console):
```
powershell -Command "iwr -useb https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1 | iex"
```
install terraform

on windows:
```
choco install terraform
```

install azure cli

on windows:
```
choco install azure-cli
```

login to azure and set the right account to deploy to:
```
az login
az account show
az account set --subscription="SUBSCRIPTION_ID"
```

#create a service principal (or do it manually in the portal):
```
az ad sp create-for-rbac --role="Contributor" --scopes="/subscriptions/SUBSCRIPTION_ID"
```

manually create a storage account in your azure subscription, used to store terraform state files
run the pre-reqs terraform first, this sets up a key vault

```
terraform init -backend-config azurerm.hcl
```

azurerm.hcl file (gitignored):
```
storage_account_name    = "STORAGE_ACCOUNT_NAME"
container_name          = "CONTAINER_NAME"
key                     = "TFSTATE_FILE_NAME.tfstate"
access_key              = "storage_account_access_key"
```

manually create an app registration, add the client id and secret into the key vault, update the secret names in the environment folder of main.
TODO: pre-reqs could include terraform for the app registrations

now apply the main terraform.

connect to the cluster:
```
az aks get-credentials --resource-group dev-rl-platform-rg --name dev-rl-platform-aks
```

inspect the cluster:
```
kubectl get deployments --all-namespaces=true
```

install dapr:
```
dapr init -k --enable-ha=true
```

login to docker
```
docker login devrlsharedacr.azurecr.io
```

cd src/sentiment/backend
```
docker build -t devrlsharedacr.azurecr.io/sentiment-backend .
docker push devrlsharedacr.azurecr.io/sentiment-backend
```

cd dapr-apps/sentiment
```
kubectl apply -f ./fast-api.yaml
kubectl rollout status deploy/fastapiapp
kubectl get svc fastapiapp
```

cleanup dapr (from dapr-apps directory):
```
kubectl delete -f .
```

To cleanup infra (from terraform directory):
```
terraform destroy 
```

This is a quick tutorial to learn dapr + k8s, taken from : https://github.com/dapr/quickstarts/tree/master/hello-kubernetes

dapr setup secret store (required for hello-k8s):
```
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
helm install redis bitnami/redis+
```

cd dapr-components
```
kubectl apply -f ./redis-state.yaml
kubectl apply -f ./redis-pubsub.yaml
```

cd dapr-apps/hello-k8s
```
kubectl apply -f ./redis.yaml
kubectl apply -f ./node.yaml
kubectl rollout status deploy/nodeapp
```

order format:
```
{"data":{"orderId":"42"}}
```

```
kubectl port-forward service/nodeapp 8080:80
```

http://localhost:8080/ports
```
curl --request POST --data "{\"data\":{\"orderId\":\"42\"}}" --header Content-Type:application/json http://localhost:8080/neworder
```

OR

```
kubectl get svc nodeapp
```

http://EXTERNAL-IP/ports

Confirm it's possible to post and get an order:
```
curl --request POST --data "{\"data\":{\"orderId\":\"42\"}}" --header Content-Type:application/json http://EXTERNAL-IP/neworder
```
```
Invoke-RestMethod -Method POST -Uri http://EXTERNAL-IP/neworder -body $json -contenttype application/json
```

```
curl http://localhost:8080/order
```
```
Invoke-RestMethod -Method GET -Uri http://<EXTERNAL-IP>/order
```

Make the dapr dashboard available on http://localhost:9999/
```
dapr dashboard -k -p 9999
```

if not still in this directory:
cd dapr-apps/hello-k8s 
```
kubectl apply -f ./python.yaml
kubectl rollout status deploy/pythonapp
```

observe messages:
```
kubectl logs --selector=app=node -c node --tail=-1
```
