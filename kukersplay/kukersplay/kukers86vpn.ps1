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

New-NetFirewallRule -DisplayName 'KukersPlay' -Profile @('Domain', 'Private', 'Public') -Action Allow -Protocol 'TCP' -Direction Inbound -LocalPort @('13100', '13200') -RemotePort @('13100', '13200')
New-NetFirewallRule -DisplayName 'KukersPlay' -Profile @('Domain', 'Private', 'Public') -Action Allow -Protocol 'TCP' -Direction Outbound -LocalPort @('13100', '13200') -RemotePort @('13100', '13200')
New-NetFirewallRule -DisplayName 'KukersPlay' -Profile @('Domain', 'Private', 'Public') -Action Allow -Protocol 'UDP' -Direction Inbound -LocalPort @('13100', '13200') -RemotePort @('13100', '13200')
New-NetFirewallRule -DisplayName 'KukersPlay' -Profile @('Domain', 'Private', 'Public') -Action Allow -Protocol 'UDP' -Direction Outbound -LocalPort @('13100', '13200') -RemotePort @('13100', '13200')
