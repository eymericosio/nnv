import {
	Card,
	CardContent,
	Typography,
	CardActions,
	Button,
	Chip,
	Box,
} from "@mui/material";
import * as React from "react";
import { IMovie } from "./Api/MoviesApi";
import { Link } from "react-router-dom";

export function MovieCard(props: { movie: IMovie }) {
	return (
		<>
			<Card>
				<CardContent>
					<Typography variant="h5" component="div">
						{props.movie.name}
					</Typography>
					<Typography sx={{ mb: 1.5 }} color="text.secondary">
						Length: {props.movie.length} | Rating: {props.movie.rate}
					</Typography>
					<Typography
						sx={{ fontSize: 14 }}
						color="text.secondary"
						gutterBottom
						component={Box}
					>
						{props.movie.genres.map((g, i) => (
							<Chip key={i} label={g} size="small" />
						))}
					</Typography>
					<Typography variant="body2">{props.movie.description}</Typography>
				</CardContent>
				<CardActions>
					<Button
						size="small"
						component={Link}
						to={"movies/" + props.movie.key}
					>
						See More
					</Button>
				</CardActions>
			</Card>
		</>
	);
}
