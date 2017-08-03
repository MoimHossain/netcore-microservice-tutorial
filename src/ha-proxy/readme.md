### Using nginx as reverse proxy to other services

This is the minimum config for nginx - running in a container that reverse-proxy two or more services running inside the cluster.

### Testing on local machine

In order to test sub-domain routing on local machine, we need to edit the `C:\Windows\System32\drivers\etc\hosts` file as the following:
```
# localhost name resolution is handled within DNS itself.
#	127.0.0.1       localhost
#	::1             localhost


# Adding some test URLs for local machine
	127.0.0.1       example.com  ui.example.com api.example.com

```

Now we can test [ui.example.com](ui.example.com) and [api.example.com](api.example.com) with a browser window running on local machine.