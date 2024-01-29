import * as React from "react";
import { MoviesList } from "./MoviesList";
import { gql, useQuery } from "@apollo/client";
import { IMovie } from "./Api/MoviesApi";
import { useMemo } from "react";

export default function TopRatedRoute() {
	const MOVIES_TOP_RATED_QUERY = useMemo(
		() => gql`
			query MoviesTopRated {
				movies_top_rated {
					key
					name
					description
					rate
					length
					img
					genres
				}
			}
		`,
		[]
	);

	const { data, error } = useQuery<{
		movies_top_rated: Array<IMovie>;
	}>(MOVIES_TOP_RATED_QUERY, { pollInterval: 30000 });

	if (error) return <pre>{error.message}</pre>;

	return (
		<>
			<MoviesList movies={data?.movies_top_rated ?? []} />
		</>
	);
}
