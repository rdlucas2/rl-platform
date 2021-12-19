module "resource_group" {
    source = "../../shared/resource-group"

    resource_group_name     = var.resource_group_name
    location                = var.location
    tags                    = var.tags
}

data "azurerm_key_vault_secret" "appId" {
    name = var.appId_secret_name
    key_vault_id = var.key_vault_id
}

data "azurerm_key_vault_secret" "appSecret" {
    name = var.appSecret_secret_name
    key_vault_id = var.key_vault_id
}

data "azurerm_container_registry" "default" {
  name                = var.acr_name
  resource_group_name = var.acr_resource_group_name
}

data "azuread_service_principal" "default" {
  application_id = data.azurerm_key_vault_secret.appId.value
}

resource "azurerm_role_assignment" "default" {
  scope                = data.azurerm_container_registry.default.id
  role_definition_name = "AcrPull"
  principal_id         = data.azuread_service_principal.default.object_id
}

module "aks_cluster" {
    source = "../../shared/aks-cluster"

    aks_cluster_name        = var.aks_cluster_name
    location                = var.location
    resource_group_name     = var.resource_group_name
    aks_cluster_dns_prefix  = var.aks_cluster_dns_prefix
    node_pool_name          = var.node_pool_name
    node_count              = var.node_count
    node_vm_size            = var.node_vm_size
    node_os_disk_size_gb    = var.node_os_disk_size_gb
    appId                   = data.azurerm_key_vault_secret.appId.value
    appSecret               = data.azurerm_key_vault_secret.appSecret.value
    tags                    = var.tags

    depends_on = [module.resource_group]
}
