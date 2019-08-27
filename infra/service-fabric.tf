resource "azurerm_service_fabric_cluster" "chat-app" {
  name                 = "chat-app-cluster"
  resource_group_name  = "${azurerm_resource_group.chat-app.name}"
  location             = "${azurerm_resource_group.chat-app.location}"
  reliability_level    = "Bronze"
  upgrade_mode         = "Automatic"
  vm_image             = "Linux"
  management_endpoint  = "https://${azurerm_public_ip.chat-app.fqdn}:19080"

  add_on_features = ["DnsService", "RepairManager"]

  node_type {
    name                 = "chats"
    instance_count       = 5 
    is_primary           = true
    client_endpoint_port = 19000
    http_endpoint_port   = 19080
    application_ports {
      start_port = 20000,
      end_port = 30000
    }
  }

  client_certificate_thumbprint = [{thumbprint = "${azurerm_key_vault_certificate.chat-app.thumbprint}", is_admin = true}]

  certificate {
    thumbprint = "${azurerm_key_vault_certificate.chat-app.thumbprint}"
    thumbprint_secondary = "${azurerm_key_vault_certificate.chat-app.thumbprint}"
    x509_store_name = "My"
  }

  fabric_settings = [{
    name = "Security",
    parameters = {
      "ClusterProtectionLevel" = "EncryptAndSign"
    }
  }]
}
