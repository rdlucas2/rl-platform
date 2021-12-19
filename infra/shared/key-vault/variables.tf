variable "resource_group_name" {
  type = string
  description = "name of the resource group to deploy to"
}

variable "location" {
  type = string
  description = "location to deploy to"
}

variable "tags" {
  type = map
  description = "tags to apply to the resource group"
}

variable "key_vault_name" {
  type = string
  description = "name of the key vault to create"
}

variable "key_vault_sku" {
  type = string
  description = "SKU of the key vault to create | Standard | Premium"
}
