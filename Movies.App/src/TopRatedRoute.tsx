import * as React from "react";
import { MoviesList } from "./MoviesList";
import { gql, useQuery } from "@apollo/client";
import { IMovie } from "./Api/MoviesApi";

const MOVIES_TOP_RATED_QUERY = gql`
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
`;

export default function TopRatedRoute() {
	const { data, loading, error, refetch } = useQuery<{
		movies_top_rated: Array<IMovie>;
	}>(MOVIES_TOP_RATED_QUERY);

	if (error) return <pre>{error.message}</pre>;

	return (
		<>
			<MoviesList movies={data?.movies_top_rated ?? []} />
		</>
	);
}
