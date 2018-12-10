Import-Module -Name VpnClient -Force:$true

$name = "Kukers86 VPN"
$address = Get-Content -Path "./server.txt"
$result = "false"

try
{
	$username = Get-Content -Path "./login.txt"
	$plainpassword = Get-Content -Path "./pass.txt"

	$vpn = Get-VpnConnection -Name $name;

	if ($vpn){
		if($vpn.ConnectionStatus -eq "Connected"){
			rasdial $name /DISCONNECT;
		}
		Remove-VpnConnection -Name $name -Force:$true
	}

	Add-VpnConnection -Name $name -ServerAddress $address -TunnelType Pptp -EncryptionLevel Required -AuthenticationMethod MSChapv2 -Force:$true -RememberCredential:$true -SplitTunneling:$false 

	$vpn = Get-VpnConnection -Name $name;
	if($vpn.ConnectionStatus -eq "Disconnected"){
		rasdial $name $username $plainpassword;
	}

	$nics = [System.Net.NetworkInformation.NetworkInterface]::GetAllNetworkInterfaces()
	foreach ($nic in $nics) {
		if ($nic.name -eq $name) {
			$props = $nic.GetIPProperties()
			$addresses = $props.UnicastAddresses
			foreach ($addr in $addresses) {
				echo "$($addr.Address.IPAddressToString)" | Out-File -FilePath "./ipaddress.txt"
				$result = "true"
			}
		}
	}
}
catch [system.exception] {
}
	
echo "$result" | Out-File -FilePath "./result.txt"