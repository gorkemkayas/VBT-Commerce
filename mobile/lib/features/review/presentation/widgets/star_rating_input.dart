import 'package:flutter/material.dart';

/// Dokunarak 1-5 arası puan seçilen düz siyah yıldız satırı. Hem yeni
/// değerlendirme formunda (`_CreateReviewForm`) hem de düzenleme formunda
/// aynen kullanılır.
class StarRatingInput extends StatelessWidget {
  const StarRatingInput({super.key, required this.rating, required this.onChanged});

  final int rating;
  final ValueChanged<int> onChanged;

  @override
  Widget build(BuildContext context) => Row(
    mainAxisSize: MainAxisSize.min,
    children: [
      for (var index = 0; index < 5; index++) ...[
        InkWell(
          onTap: () => onChanged(index + 1),
          borderRadius: BorderRadius.circular(4),
          child: Icon(
            index < rating ? Icons.star : Icons.star_border,
            size: 20,
            color: Colors.black,
          ),
        ),
        if (index != 4) const SizedBox(width: 2),
      ],
    ],
  );
}
