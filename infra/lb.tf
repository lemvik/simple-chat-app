resource "azurerm_lb" "chat-app" { # load balancer
  name                = "chat-app-lb"
  location            = "${azurerm_resource_group.chat-app.location}"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"

  frontend_ip_configuration {
    name                 = "PublicIPAddress-SF"
    public_ip_address_id = "${azurerm_public_ip.chat-app.id}"
  }
}

resource "azurerm_lb_nat_pool" "chat-app" { # nat pool for load balancer
  name                           = "chat-app-nat-pool"
  resource_group_name            = "${azurerm_resource_group.chat-app.name}"
  loadbalancer_id                = "${azurerm_lb.chat-app.id}"
  count                          = 3
  protocol                       = "Tcp"
  frontend_port_start            = 3389
  frontend_port_end              = 4500
  backend_port                   = 3389
  frontend_ip_configuration_name = "PublicIPAddress-SF"
}

resource "azurerm_lb_backend_address_pool" "chat-app" { # load balancer address pool
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  loadbalancer_id     = "${azurerm_lb.chat-app.id}"
  name                = "ServiceFabricAddressPool"
}

# Probes
resource "azurerm_lb_probe" "fabric_gateway" { # SF client endpoint port.
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  loadbalancer_id     = "${azurerm_lb.chat-app.id}"
  name                = "chat-app-probe-19000"
  port                = 19000
}

resource "azurerm_lb_probe" "http" { # SF client http endpoint port.
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  loadbalancer_id     = "${azurerm_lb.chat-app.id}"
  name                = "chat-app-probe-19080"
  port                = 19080
}

resource "azurerm_lb_probe" "app_port_0" { # http - purpose?
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  loadbalancer_id     = "${azurerm_lb.chat-app.id}"
  name                = "chat-app-probe-80"
  port                = 80
}

resource "azurerm_lb_probe" "app_port_1" {
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  loadbalancer_id     = "${azurerm_lb.chat-app.id}"
  name                = "chat-app-probe-83"
  port                = 83
}

resource "azurerm_lb_rule" "app_port_0" {
  resource_group_name            = "${azurerm_resource_group.chat-app.name}"
  loadbalancer_id                = "${azurerm_lb.chat-app.id}"
  backend_address_pool_id        = "${azurerm_lb_backend_address_pool.chat-app.id}"
  probe_id                       = "${azurerm_lb_probe.app_port_0.id}"
  name                           = "AppPortLBRule0"
  protocol                       = "Tcp"
  frontend_port                  = 80
  backend_port                   = 80
  frontend_ip_configuration_name = "PublicIPAddress-SF"
}

resource "azurerm_lb_rule" "app_port_1" {
  resource_group_name            = "${azurerm_resource_group.chat-app.name}"
  loadbalancer_id                = "${azurerm_lb.chat-app.id}"
  backend_address_pool_id        = "${azurerm_lb_backend_address_pool.chat-app.id}"
  probe_id                       = "${azurerm_lb_probe.app_port_1.id}"
  name                           = "AppPortLBRule1"
  protocol                       = "Tcp"
  frontend_port                  = 83
  backend_port                   = 83
  frontend_ip_configuration_name = "PublicIPAddress-SF"
}

resource "azurerm_lb_rule" "http" {
  resource_group_name            = "${azurerm_resource_group.chat-app.name}"
  loadbalancer_id                = "${azurerm_lb.chat-app.id}"
  backend_address_pool_id        = "${azurerm_lb_backend_address_pool.chat-app.id}"
  probe_id                       = "${azurerm_lb_probe.http.id}"
  name                           = "http"
  protocol                       = "Tcp"
  frontend_port                  = 19080
  backend_port                   = 19080
  frontend_ip_configuration_name = "PublicIPAddress-SF"
}

resource "azurerm_lb_rule" "fabric_gateway" {
  resource_group_name            = "${azurerm_resource_group.chat-app.name}"
  loadbalancer_id                = "${azurerm_lb.chat-app.id}"
  backend_address_pool_id        = "${azurerm_lb_backend_address_pool.chat-app.id}"
  probe_id                       = "${azurerm_lb_probe.fabric_gateway.id}"
  name                           = "fabric_gateway"
  protocol                       = "Tcp"
  frontend_port                  = 19000
  backend_port                   = 19000
  frontend_ip_configuration_name = "PublicIPAddress-SF"
}
