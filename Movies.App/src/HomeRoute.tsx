import { Grid, Chip, TextField, Autocomplete, Box, Fab } from "@mui/material";
import * as React from "react";
import { useMemo, useState } from "react";
import { MoviesList } from "./MoviesList";
import { gql, useQuery } from "@apollo/client";
import { IMovie } from "./Api/MoviesApi";
import AddIcon from "@mui/icons-material/Add";
import { useNavigate } from "react-router-dom";

export default function HomeRoute() {
	const [search, setSearch] = useState<{
		text: string;
		genres: Array<string>;
	}>({
		text: "",
		genres: [],
	});
	const navigate = useNavigate();

	const MOVIES_QUERY = useMemo(
		() => gql`
			query Movies($text: String, $genres: [String]) {
				movies(text: $text, genres: $genres) {
					key
					name
					rate
					genres
					length
				}
			}
		`,
		[]
	);

	const { data, error } = useQuery<{ movies: Array<IMovie> }>(MOVIES_QUERY, {
		variables: { text: search.text, genres: search.genres },
	});

	if (error) return <pre>{error.message}</pre>;

	return (
		<>
			<Grid container spacing={2} sx={{ marginBottom: 5 }}>
				<Grid item xs={5}>
					<TextField
						label="Search name and description..."
						fullWidth
						value={search.text}
						onChange={(event) => {
							setSearch({ ...search, text: event.target.value });
						}}
					/>
				</Grid>
				<Grid item xs={7}>
					<Autocomplete
						multiple
						options={[]}
						defaultValue={[]}
						freeSolo
						value={search.genres}
						onChange={(event, value) => {
							setSearch({ ...search, genres: value });
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

			<Box sx={{ position: "sticky", bottom: 16, marginTop: 15 }}>
				<Fab
					color="primary"
					aria-label="add"
					sx={{ position: "absolute", bottom: 0, right: 16 }}
					onClick={() => {
						navigate("/create");
					}}
				>
					<AddIcon />
				</Fab>
			</Box>
		</>
	);
}
