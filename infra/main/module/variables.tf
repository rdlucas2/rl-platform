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

variable "aks_cluster_name" {
  type = string
  description = "name of the AKS cluster"
}

variable "aks_cluster_dns_prefix" {
  type = string
  description = "AKS cluster DNS prefix"
}

variable "node_pool_name" {
  type = string
  description = "name of the node pool"
}

variable "node_count" {
  type = number
  description = "number of nodes in the node pool"
}

variable "node_vm_size" {
  type = string
  description = "size of the node VMs"
}

variable "node_os_disk_size_gb" {
  type = number
  description = "size of the node OS disk in GB"
}

variable "appId_secret_name" {
  type = string
  description = "name of the appId secret in the key vault"
}

variable "appSecret_secret_name" {
  type = string
  description = "name of the appSecret secret in the key vault"
}

variable "key_vault_id" {
  type = string
  description = "id of the key vault"
}

variable "acr_name" {
  type = string
  description = "value of the acr name"
}

variable "acr_resource_group_name" {
  type = string
  description = "value of the acr resource group name"
}
