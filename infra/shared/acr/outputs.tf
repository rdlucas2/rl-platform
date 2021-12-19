output "acr_id" {
  value = azurerm_container_registry.default.id
}

output "acr_login_server" {
  value = azurerm_container_registry.default.login_server
}

output "acr_admin_username" {
  value = azurerm_container_registry.default.admin_username
}

output "acr_admin_password" {
  value = azurerm_container_registry.default.admin_password
}

output "acr_identity" {
  value = azurerm_container_registry.default.identity
}
