import { Grid, Chip, TextField, Autocomplete } from "@mui/material";
import * as React from "react";
import { useState } from "react";
import { MoviesList } from "./MoviesList";
import { gql, useQuery } from "@apollo/client";
import { IMovie } from "./Api/MoviesApi";

const MOVIES_QUERY = gql`
	query Movies($text: String, $genres: [String]) {
		movies(search: $text, genres: $genres) {
			key
			name
			rate
			genres
			length
		}
	}
`;

export default function HomeRoute() {
	const [inputs, setSearch] = useState<{
		text: string;
		genres: Array<string>;
	}>({
		text: "",
		genres: [],
	});

	const { data, loading, error, refetch } = useQuery<{ movies: Array<IMovie> }>(
		MOVIES_QUERY,
		{ variables: { text: inputs.text, genres: inputs.genres } }
	);

	if (error) return <pre>{error.message}</pre>;

	return (
		<>
			<Grid container spacing={2} sx={{ marginBottom: 5 }}>
				<Grid item xs={5}>
					<TextField
						label="Search name and description..."
						fullWidth
						value={inputs.text}
						onChange={(event) => {
							setSearch({ ...inputs, text: event.target.value });
						}}
					/>
				</Grid>
				<Grid item xs={7}>
					<Autocomplete
						multiple
						options={[]}
						defaultValue={[]}
						freeSolo
						value={inputs.genres}
						onChange={(event, value) => {
							setSearch({ ...inputs, genres: value });
						}}
						renderTags={(value: readonly string[], getTagProps) =>
							value.map((option: string, index: number) => (
								<Chip label={option} {...getTagProps({ index })} />
							))
						}
						renderInput={(params) => (
							<TextField {...params} label="Search by genres..." />
						)}
					/>
				</Grid>
			</Grid>

			<MoviesList movies={data?.movies ?? []} />
		</>
	);
}
