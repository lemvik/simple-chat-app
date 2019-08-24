resource "azurerm_service_fabric_cluster" "chat-app" {
  name                 = "chat-app-cluster"
  resource_group_name  = "${azurerm_resource_group.chat-app.name}"
  location             = "${azurerm_resource_group.chat-app.location}"
  reliability_level    = "Bronze"
  upgrade_mode         = "Automatic"
  vm_image             = "Linux"
  management_endpoint  = "https://chat-app.cloudapp.net:19080"

  node_type {
    name                 = "chats"
    instance_count       = 3 
    is_primary           = true
    client_endpoint_port = 2020
    http_endpoint_port   = 80
  }
}
