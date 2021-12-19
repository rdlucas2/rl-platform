module "resource_group" {
    source = "../../shared/resource-group"

    resource_group_name     = var.resource_group_name
    location                = var.location
    tags                    = var.tags
}

module "key_vault" {
    source = "../../shared/key-vault"

    key_vault_name          = var.key_vault_name
    location                = var.location
    resource_group_name     = var.resource_group_name
    key_vault_sku           = var.key_vault_sku

    tags                    = var.tags

    depends_on              = [module.resource_group]
}

module "acr" {
    source = "../../shared/acr"

    acr_name                = var.acr_name
    location                = var.location
    resource_group_name     = var.resource_group_name
    acr_sku                 = var.acr_sku
    acr_admin_enabled       = var.acr_admin_enabled

    tags                    = var.tags

    depends_on              = [module.resource_group]
}
