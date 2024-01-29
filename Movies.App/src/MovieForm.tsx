import { Grid, Chip, TextField, Autocomplete } from "@mui/material";
import * as React from "react";
import { IMovie } from "./Api/MoviesApi";

export default function MovieForm(props: {
	movie: IMovie;
	setMovie: (movie: IMovie) => void;
	disabled: boolean;
}) {
	return (
		<>
			<Grid container spacing={4}>
				<Grid item xs={6}>
					<TextField
						label="Name"
						value={props.movie.name}
						onChange={(event) =>
							props.setMovie({ ...props.movie, name: event.target.value })
						}
						fullWidth
						disabled={props.disabled}
					/>
				</Grid>

				<Grid item xs={6}>
					<TextField
						label="Rate"
						type="number"
						inputProps={{ step: ".1" }}
						value={props.movie.rate}
						onChange={(event) =>
							props.setMovie({
								...props.movie,
								rate: Number(event.target.value),
							})
						}
						fullWidth
						disabled={props.disabled}
					/>
				</Grid>

				<Grid item xs={6}>
					<TextField
						label="Duration"
						value={props.movie.length}
						onChange={(event) =>
							props.setMovie({ ...props.movie, length: event.target.value })
						}
						fullWidth
						disabled={props.disabled}
					/>
				</Grid>

				<Grid item xs={6}>
					<TextField
						label="Image"
						value={props.movie.img}
						onChange={(event) =>
							props.setMovie({ ...props.movie, img: event.target.value })
						}
						fullWidth
						disabled={props.disabled}
					/>
				</Grid>

				<Grid item xs={12}>
					<Autocomplete
						multiple
						options={[]}
						defaultValue={[]}
						freeSolo
						value={props.movie.genres}
						onChange={(event, values) =>
							props.setMovie({ ...props.movie, genres: [...values] })
						}
						renderTags={(value: readonly string[], getTagProps) =>
							value.map((option: string, index: number) => (
								<Chip label={option} {...getTagProps({ index })} />
							))
						}
						renderInput={(params) => <TextField {...params} label="Genres" />}
						disabled={props.disabled}
					/>
				</Grid>

				<Grid item xs={12}>
					<TextField
						label="Description"
						value={props.movie.description}
						onChange={(event) =>
							props.setMovie({
								...props.movie,
								description: event.target.value,
							})
						}
						fullWidth
						multiline
						disabled={props.disabled}
					/>
				</Grid>
			</Grid>
		</>
	);
}
