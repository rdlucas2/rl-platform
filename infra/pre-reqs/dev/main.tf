terraform {
  backend "azurerm" {}
}

provider "azurerm" {
  features {}
}

module "dev_shared" {
  source = "../module"
    
    resource_group_name     = "dev-rl-shared-rg"
    location                = "eastus2"
    key_vault_name          = "dev-rl-shared-kv" 
    key_vault_sku           = "standard"
    acr_name                = "devrlsharedacr"
    acr_sku                 = "Basic"
    acr_admin_enabled       = true

    tags = {
      environment = "dev"
      application = "shared"
      owner       = "rdlucas2@gmail.com"
    }
}