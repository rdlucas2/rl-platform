resource "azurerm_kubernetes_cluster" "default" {
  name                = var.aks_cluster_name
  location            = var.location
  resource_group_name = var.resource_group_name
  dns_prefix          = var.aks_cluster_dns_prefix

  default_node_pool {
    name              = var.node_pool_name
    node_count        = var.node_count
    vm_size           = var.node_vm_size
    os_disk_size_gb   = var.node_os_disk_size_gb
  }

  service_principal {
    client_id         = var.appId
    client_secret     = var.appSecret
  }

  role_based_access_control {
    enabled           = true
  }

  tags                = var.tags
}
