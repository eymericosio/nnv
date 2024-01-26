import * as React from "react";
import { useMemo } from "react";
import { useAuth } from "oidc-react";
import { ApolloProvider, ApolloClient, InMemoryCache } from "@apollo/client";
import * as config from "./config.json";
import Router from "./Router";

export default function App() {
	const auth = useAuth();

	const gqlClient = useMemo(() => {
		return new ApolloClient({
			uri: config.GraphQLRootUrl,
			cache: new InMemoryCache({
				typePolicies: {
					movie: { keyFields: ["key"] },
				},
			}),
			credentials: "include",
			connectToDevTools: true, // for debug purpose
			headers: {
				authorization: "Bearer " + auth.userData?.access_token,
			},
		});
	}, [auth.userData?.access_token]);

	if (auth.isLoading || auth.userData == null) return <></>;

	return (
		<>
			<ApolloProvider client={gqlClient}>
				<Router />
			</ApolloProvider>
		</>
	);
}
