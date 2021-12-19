output "key_vault_id" {
  value = azurerm_key_vault.default.id
}

output "resource_group_vault_uri" {
  value = azurerm_key_vault.default.vault_uri
}
