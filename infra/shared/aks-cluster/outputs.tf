output "aks_cluster_id" {
  value = azurerm_kubernetes_cluster.default.id
}

output "aks_cluster_fqdn" {
  value = azurerm_kubernetes_cluster.default.fqdn
}

output "aks_cluster_private_fqdn" {
  value = azurerm_kubernetes_cluster.default.private_fqdn
}

output "aks_cluster_portal_fqdn" {
  value = azurerm_kubernetes_cluster.default.portal_fqdn
}

output "aks_cluster_kube_admin_config" {
  value = azurerm_kubernetes_cluster.default.kube_admin_config
}
