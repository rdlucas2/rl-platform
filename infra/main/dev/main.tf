terraform {
  backend "azurerm" {}
}

provider "azurerm" {
  features {}
}

data "azurerm_key_vault" "default" {
  name                = "dev-rl-shared-kv"
  resource_group_name = "dev-rl-shared-rg"
}

module "dev_platform" {
  source = "../module"
    
    resource_group_name     = "dev-rl-platform-rg"
    location                = "eastus2"
    aks_cluster_name        = "dev-rl-platform-aks"
    aks_cluster_dns_prefix  = "dev-rl-platform-k8s"
    node_pool_name          = "default"
    node_count              = "2"
    node_vm_size            = "Standard_D2_v2"
    node_os_disk_size_gb    = 30
    appId_secret_name       = "dev-rl-platform-appId"
    appSecret_secret_name   = "dev-rl-platform-appSecret"
    key_vault_id            = data.azurerm_key_vault.default.id
    acr_name                = "devrlsharedacr" 
    acr_resource_group_name = "dev-rl-shared-rg"

    tags = {
      environment = "dev"
      application = "platform"
      owner       = "rdlucas2@gmail.com"
    }
}