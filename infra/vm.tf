resource "azurerm_virtual_machine_scale_set" "chat-app" {
  name                = "chat-app-scale-set"
  location            = "${azurerm_resource_group.chat-app.location}"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  upgrade_policy_mode = "Automatic"
  overprovision       = false

  sku {
    name     = "Standard_B1s"
    tier     = "Standard"
    capacity = 4
  }

  storage_profile_image_reference {
    publisher = "Canonical"
    offer     = "UbuntuServer"
    sku       = "16.04-LTS"
    version   = "latest"
  }

  storage_profile_os_disk {
    name              = ""
    caching           = "ReadWrite"
    create_option     = "FromImage"
    managed_disk_type = "Standard_LRS"
  }

  storage_profile_data_disk {
    lun            = 0
    caching        = "ReadWrite"
    create_option  = "Empty"
    disk_size_gb   = 5 
  }

  os_profile {
    computer_name_prefix = "chat-app"
    admin_username       = "lemvic"
    admin_password       = "SomeDumbPassw0rd11!"
  }

  os_profile_secrets {
    source_vault_id = "${azurerm_key_vault.chat-app.id}"
    vault_certificates {
      certificate_url = "${azurerm_key_vault_certificate.chat-app.secret_id}"
    }
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    name    = "terraformnetworkprofile"
    primary = true

    ip_configuration {
      primary = true
      name                                   = "ChatAppIPConfiguration"
      subnet_id                              = "${azurerm_subnet.chat-app-subnet.id}"
      load_balancer_backend_address_pool_ids = ["${azurerm_lb_backend_address_pool.chat-app.id}"]
      load_balancer_inbound_nat_rules_ids    = ["${element(azurerm_lb_nat_pool.chat-app.*.id, count.index)}"]
    }
  }

  extension {
    name                 = "MSILinuxExtension"
    publisher            = "Microsoft.ManagedIdentity"
    type                 = "ManagedIdentityExtensionForLinux"
    type_handler_version = "1.0"
    settings             = "{\"port\": 50342}"
  }

  extension { 
    name                 = "ServiceFabricNodeVmExt_vmchats"
    publisher            = "Microsoft.Azure.ServiceFabric"
    type                 = "ServiceFabricLinuxNode"
    type_handler_version = "1.1"
    settings             = "{ \"certificate\" : {\"thumbprint\": \"${azurerm_key_vault_certificate.chat-app.thumbprint}\", \"x509StoreName\":\"My\"},  \"clusterEndpoint\": \"${azurerm_service_fabric_cluster.chat-app.cluster_endpoint}\", \"nodeTypeRef\": \"chats\", \"durabilityLevel\": \"Bronze\",\"nicPrefixOverride\": \"10.0.0.0/24\"}"
  }
}
