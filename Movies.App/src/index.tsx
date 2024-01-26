import "@fontsource/roboto/300.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";
import "@fontsource/roboto/700.css";
import * as React from "react";
import * as ReactDOM from "react-dom/client";
import App from "./App";
import { CssBaseline } from "@mui/material";
import * as config from "./config.json";
import { AuthProvider } from "oidc-react";

ReactDOM.createRoot(document.getElementById("root")!).render(
	<React.StrictMode>
		<CssBaseline />
		<AuthProvider
			autoSignIn
			autoSignOut
			automaticSilentRenew
			authority={config.OpenIdConnect.Authority}
			clientId={config.OpenIdConnect.ClientId}
			scope="openid profile email api offline_access"
			redirectUri={config.OpenIdConnect.RedirectUri}
			onSignIn={() => {
				window.history.replaceState({}, document.title, "/");
			}}
		>
			<App />
		</AuthProvider>
	</React.StrictMode>
);
