Import-Module -Name VpnClient -Force:$true

$name = "Hamachi"
$result = "false"

try
{
	$nics = [System.Net.NetworkInformation.NetworkInterface]::GetAllNetworkInterfaces()
	foreach ($nic in $nics) {
		if ($nic.name -contains $name) {
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