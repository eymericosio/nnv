import { Grid } from "@mui/material";
import * as React from "react";
import { IMovie } from "./Api/MoviesApi";
import { MovieCard } from "./MovieCard";

export function MoviesList(props: { movies: Array<IMovie> }) {
	return (
		<>
			<Grid container spacing={2}>
				{props.movies.map((m: IMovie) => (
					<Grid item xs={4} key={m.key}>
						<MovieCard movie={m} />
					</Grid>
				))}
			</Grid>
		</>
	);
}
