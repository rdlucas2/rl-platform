output "resource_group_name" {
  value = azurerm_resource_group.default.name
}

output "resource_group_location" {
  value = azurerm_resource_group.default.location
}

output "resource_group_tags" {
  value = azurerm_resource_group.default.tags
}

output "resource_group_id" {
  value = azurerm_resource_group.default.id
}
