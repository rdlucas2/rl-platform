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

variable "appId" {
  type = string
  description = "Azure Kubernetes Service Cluster service principal"
}

variable "appSecret" {
  type = string
  description = "Azure Kubernetes Service Cluster service principal secret"
}
